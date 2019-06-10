var typea = true;
var names = [];

function te_callback(resp)
{
	if (resp.id === 'evalItemId')
		parse_list(resp);
	else if (resp.id === 'studId')
		solve_namelist(resp);
	else
		handle_one(resp);
}

function solve_namelist(resp)
{
	for(var p = 0; p < resp.value.length; p++)
    {
		names.push(resp.value[p].name);
	}

	invokeCSharpAction('post;service/res.do;{"tag":"student@evalItem","branch":"self","params":{"blank":"Y"}}');
}

function solve()
{
	if (i >= len) return;
	$('#'+list[i]).removeClass('table-primary').addClass('table-info');
	invokeCSharpAction('post;action/eval/fetch-eval-item.do;{"evalItemId":"' + list[i] + '"}');
}

function handle_one(resp)
{
	if (resp.count === 1 && resp.items[0].puzzle !== undefined)
	{
		var current = resp.items[0].puzzle.split('');
		var ans = '';
		for(var p = 0; p < names.length; p++)
		{
			var tocheck = names[p].split('');
			if (current.length === tocheck.length)
			{
				var match = true;
				for (var t = 0; t < names.length; t++)
				{
					if (tocheck[t] !== '_' && tocheck[t] !== current[t])
					{
						match = false;
						break;
					}
					else if (tocheck[t] === '_')
					{
						ans = current[t];
					}
				}
				if (match) break;
			}
		}

		invokeCSharpAction('msg;' + ans);

		if (ans === '')
		{
			invokeCSharpAction('msg;emmmm, something went wrong in name finding... I am sorry~');
			invokeCSharpAction('finished');
		}
		else
		{
			invokeCSharpAction('post;action/eval/eval-with-answer.do;{"guidelineId":120,"evalItemId":"' + list[i] + '","answers":{"prob11":"A","prob12":"A","prob13":"N","prob14":"A","prob15":"A","prob21":"A","prob22":"A","prob23":"A","prob31":"A","prob32":"A","prob33":"A","prob41":"A","prob42":"A","prob43":"A","prob51":"A","prob52":"A","sat6":"A","mulsel71":"K","advice72":"good","prob73":"Y","puzzle_answer":"' + ans +'"},"clicks":{"_boot_":0,"prob11":129550,"prob12":131673,"prob13":134548,"prob14":137761,"prob15":140810,"prob21":143057,"prob22":145055,"prob23":146495,"prob31":150531,"prob32":151706,"prob33":152729,"prob41":154707,"prob42":155872,"prob43":160394,"prob51":163353,"prob52":165352,"sat6":166962,"mulsel71":171192,"prob73":176278}}');
		}
	}
	else if (resp.count !== 1)
	{
		$('#'+list[i]).removeClass('table-primary').addClass('table-warning');
        invokeCSharpAction('msg;' + resp.msg);
        invokeCSharpAction('finished');
	}
	else
	{
		$('#'+list[i]).removeClass('table-info').addClass('table-success');
		i++;
		solve();
	}
}

function parse_list(resp)
{
    for(var p = 0; p < resp.value.length; p++)
    {
        $('#evalItemList').append('<tr id="' + resp.value[p].evalItemId + '" class="' + (typea ? resp.value[p].evalActTime.evalGuideline.evalGuidelineId === '120' ? 'table-primary' : 'table-warning' : 'table-success') + '"><td>' + resp.value[p].target.name + '</td><td>' + resp.value[p].target.school.schoolName + '</td>' + (uwp ? '<td>' + resp.value[p].targetClar.notes + '</td>' : '') + '</tr>');
        if (resp.value[p].evalActTime.evalGuideline.evalGuidelineId === '120')
        {
            len = list.push(resp.value[p].evalItemId);
        }
    }
	if (typea)
	{
		invokeCSharpAction('post;service/res.do;{"tag":"student@evalItem","branch":"self","params":{"done":"Y"}}');
		typea = false;
	}
	else
	{
		invokeCSharpAction('finished');
	}
}

$(function(){
	invokeCSharpAction('begin');
	invokeCSharpAction('post;service/res.do;{"tag":"student_sch_dept","branch":"default","params":{"adcId":"`adcId`"}}');
});