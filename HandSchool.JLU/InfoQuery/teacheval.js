var typea = true;
var names = [];
var guidelineId = 160;

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
					if (current[t] !== '_' && tocheck[t] !== current[t])
					{
						match = false;
						break;
					}
					else if (current[t] === '_')
					{
						ans = tocheck[t];
					}
				}
				if (match) break;
			}
		}

		if (ans === '')
		{
			invokeCSharpAction('msg;emmmm, something went wrong in name finding... I am sorry~');
			invokeCSharpAction('finished');
		}
		else
		{
			invokeCSharpAction('post;action/eval/eval-with-answer.do;{"guidelineId":' + guidelineId + ',"evalItemId":"' + list[i] + '","answers":{"p01":"A","p02":"A","p03":"A","p04":"A","p05":"A","p06":"A","p07":"A","p08":"A","p09":"A","p10":"A","sat11":"A","sat12":"A","puzzle_answer":"' + ans +'"},"clicks":{"_boot_":0,"p01":564159,"p02":565357,"p03":566910,"p04":568627,"p05":570216,"p06":572475,"p07":574019,"p08":575632,"p09":577687,"p10":579937,"sat11":581460,"sat12":582592}}');
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
        $('#evalItemList').append('<tr id="' + resp.value[p].evalItemId + '" class="' + (typea ? resp.value[p].evalActTime.evalGuideline.evalGuidelineId === guidelineId ? 'table-primary' : 'table-warning' : 'table-success') + '"><td>' + resp.value[p].target.name + '</td><td>' + resp.value[p].target.school.schoolName + '</td>' + (uwp ? '<td>' + resp.value[p].targetClar.notes + '</td>' : '') + '</tr>');
        if (resp.value[p].evalActTime.evalGuideline.evalGuidelineId === guidelineId)
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