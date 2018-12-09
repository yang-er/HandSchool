
var splanId = "0";
var lslId = 0;

function showDetail(id)
{
	invokeCSharpAction('begin');
	$('#schList').html('');
	lslId = id;
	invokeCSharpAction('post;service/res.do;{"tag":"lessonSelectLogTcm@selectGlobalStore","branch":"self","params":{"lslId":'+id+',"myCampus":"Y"}}');
}

function responseSplanId(resp) /* Get current select course plan */
{
	if (resp.value.length === 0)
	{
		invokeCSharpAction('finished');
		invokeCSharpAction('msg;目前选课暂无活动的选课计划。');
	}
	else if (resp.value.length > 1)
	{
		invokeCSharpAction('finished');
		invokeCSharpAction('msg;目前活动的选课计划超过1个，可能出现错误，请手动选课。');
	}
	else
	{
		$('#splanName').text(resp.value[0].title);
		splanId = resp.value[0].splanId;
		invokeCSharpAction('post;service/res.do;{"tag":"lessonSelectLog@selectStore","branch":"default","params":{"splanId":"'+splanId+'"}}');
	}
}

function responseLslId(resp) /* List all the course current */
{
	for (var i = 0; i < resp.value.length; i++)
	{
		var ret = '<tr><td data-lslid="'+resp.value[i].lslId+'">';
		ret += resp.value[i].selectResult==='Y' ? '<span style="color:green">已选</span>' : '<span style="color:red">未选</span>';
		ret += '</td><td>';
		ret += '<a class="linked-a" onclick="showDetail('+resp.value[i].lslId+')">'+resp.value[i].lesson.courseInfo.courName+'</a>';
		ret += '</td><td>';
		ret += resp.value[i].applyPlanLesson.selectType === 3060 ? '必修课' : resp.value[i].applyPlanLesson.selectType === 3064 ? '体育课' : '选修课';
		ret += '</td></tr>';
		$('#courList').append(ret);
	}
	invokeCSharpAction('finished');
}

function switchSelect(obj)
{
	invokeCSharpAction('begin');
	invokeCSharpAction('post;action/select/select-lesson.do;{"lsltId":"'+$(obj).data('lsltid')+'","opType":"'+($(obj).data('selecttag') === 'Y' ? 'N' : 'Y')+'"}');
}

function responseSelectLesson(resp)
{
	if (resp.value.count === 1)
	{
		var jqObj = $('a[data-lsltid="'+resp.send.lsltId+'"]');
		jqObj.data('selecttag', resp.send.opType);
		jqObj.text(resp.send.opType==='Y'?'退选':'选课');
		$('td[data-lslid="'+jqObj.data('lslid')+'"]').html(resp.send.opType==='Y' ? '<span style="color:green">已选</span>' : '<span style="color:red">未选</span>');
		invokeCSharpAction('finished');
		invokeCSharpAction('msg;'+(resp.send.opType==='Y'?'选':'退选')+'课成功！');
	}
	else
	{
		invokeCSharpAction('finished');
		invokeCSharpAction('msg;'+resp.value.msg);
	}
}

function responseLsltId(resp) /* List all the course schedule */
{
	for (var i = 0; i < resp.value.length; i++)
	{
		var ret = '<tr><td>';
		ret += resp.value[i].teachClassMaster.lessonTeachers[0].teacher.name;
		if (resp.value[i].adviceTag === "A") ret += '(推荐)';
		ret += '</td><td>';
		ret += resp.value[i].selectTag!=='G'?'<a class="linked-a" data-lslid="'+lslId+'" data-lsltid="'+resp.value[i].lsltId+'" data-selecttag="'+resp.value[i].selectTag+'" onclick="switchSelect(this)">'+(resp.value[i].selectTag==='Y'?'退选':'选课')+'</a>':'固定';
		ret += '</td><td>';
		for (var j = 0; j < resp.value[i].teachClassMaster.lessonSchedules.length; j++)
		{
			if (j > 0) ret += '<br>';
			ret += resp.value[i].teachClassMaster.lessonSchedules[j].timeBlock.name + ' ' + resp.value[i].teachClassMaster.lessonSchedules[j].classroom.fullName;
		}
		ret += '</td></tr>';
		$('#schList').append(ret);
	}
	invokeCSharpAction('finished');
}

function te_callback(resp)
{
	if (resp.resName === 'query-splan-by-stud')
	{
		responseSplanId(resp);
	}
	else if (resp.id === 'lslId')
	{
		responseLslId(resp);
	}
	else if (resp.id === 'lsltId')
	{
		responseLsltId(resp);
	}
	else if (resp.id === 'selectlesson')
	{
		responseSelectLesson(resp);
	}
	else
	{
		invokeCSharpAction('msg;出现错误：未知的响应。');
	}
}

$(function(){
	invokeCSharpAction('begin');
	invokeCSharpAction('post;service/res.do;{"type":"query","res":"query-splan-by-stud","params":{"studId":`studId`}}');
});
